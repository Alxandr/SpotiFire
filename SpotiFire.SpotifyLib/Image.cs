using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Linq.Expressions;
using System.Threading;

namespace SpotiFire.SpotifyLib
{
    public delegate void ImageEventHandler(Image sender, EventArgs e);
    public class Image : DisposeableSpotifyObject
    {
        #region Delegates
        private delegate void image_loaded_cb(IntPtr imagePtr, IntPtr userdataPtr);
        #endregion

        #region Declarations
        internal IntPtr imagePtr = IntPtr.Zero;
        private Session session;
        private image_loaded_cb image_loaded;
        #endregion

        #region Constructor
        internal Image(Session session, IntPtr imagePtr)
        {
            if (imagePtr == IntPtr.Zero)
                throw new ArgumentException("imagePtr can't be zero.");

            if (session == null)
                throw new ArgumentNullException("Session can't be null.");
            this.session = session;
            this.imagePtr = imagePtr;
            this.image_loaded = new image_loaded_cb(ImageLoadedCallback);
            lock (libspotify.Mutex)
            {
                libspotify.sp_image_add_ref(imagePtr);
                libspotify.sp_image_add_load_callback(imagePtr, Marshal.GetFunctionPointerForDelegate(image_loaded), IntPtr.Zero);
            }
        }
        #endregion

        #region Properties
        public bool IsLoaded
        {
            get
            {
                lock (libspotify.Mutex)
                    return libspotify.sp_image_is_loaded(imagePtr);
            }
        }

        public sp_error Error
        {
            get
            {
                lock (libspotify.Mutex)
                    return libspotify.sp_image_error(imagePtr);
            }
        }

        public sp_imageformat Format
        {
            get
            {
                lock (libspotify.Mutex)
                    return libspotify.sp_image_format(imagePtr);
            }
        }

        public byte[] Data
        {
            get
            {
                try
                {
                    IntPtr lengthPtr = IntPtr.Zero;
                    IntPtr dataPtr = IntPtr.Zero;
                    lock (libspotify.Mutex)
                        dataPtr = libspotify.sp_image_data(imagePtr, out lengthPtr);

                    int length = lengthPtr.ToInt32();

                    if (dataPtr == IntPtr.Zero)
                        return null;
                    if (length == 0)
                        return new byte[0];

                    byte[] imageData = new byte[length];
                    Marshal.Copy(dataPtr, imageData, 0, length);
                    return imageData;
                }
                catch
                {
                    return null;
                }
            }
        }

        public string ImageId
        {
            get
            {
                lock (libspotify.Mutex)
                    return libspotify.ImageIdToString(libspotify.sp_image_image_id(imagePtr));
            }
        }
        #endregion

        #region Private Methods
        private static Delegate CreateDelegate<T>(Expression<Func<Image, Action<T>>> expr, Image s) where T : EventArgs
        {
            return expr.Compile().Invoke(s);
        }
        private void ImageLoadedCallback(IntPtr imagePtr, IntPtr userdataPtr)
        {
            if (imagePtr == this.imagePtr)
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<EventArgs>(i => i.OnLoaded, this), new EventArgs()));
        }
        #endregion

        #region Protected Methods
        protected virtual void OnLoaded(EventArgs args)
        {
            if (this.Loaded != null)
                this.Loaded(this, args);
        }
        #endregion

        #region Events
        public event ImageEventHandler Loaded;
        #endregion

        #region Cleanup
        protected override void OnDispose()
        {
            lock (libspotify.Mutex)
            {
                libspotify.sp_image_remove_load_callback(imagePtr, Marshal.GetFunctionPointerForDelegate(image_loaded), IntPtr.Zero);
                libspotify.sp_image_release(imagePtr);
            }

            imagePtr = IntPtr.Zero;
        }
        #endregion

        #region Static Public Methods
        public static Image FromId(Session session, string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            if (id.Length != 40)
                throw new ArgumentException("invalid id", "id");

            byte[] idArray = libspotify.StringToImageId(id);

            if (idArray.Length != 20)
                throw new Exception("Internal error in FromId");

            IntPtr imagePtr = IntPtr.Zero;
            IntPtr idPtr = IntPtr.Zero;
            Image image = null;
            try
            {
                idPtr = Marshal.AllocHGlobal(idArray.Length);
                Marshal.Copy(idArray, 0, idPtr, idArray.Length);

                lock (libspotify.Mutex)
                    imagePtr = libspotify.sp_image_create(session.sessionPtr, idPtr);

                image = new Image(session, imagePtr);

                lock (libspotify.Mutex)
                    libspotify.sp_image_release(imagePtr);

                return image;
            }
            catch
            {
                if(imagePtr != IntPtr.Zero)
                    try
                    {
                        lock (libspotify.Mutex)
                            libspotify.sp_image_release(imagePtr);
                    }
                    catch { }
                return null;
            }
            finally
            {
                if (idPtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(idPtr);
            }
        }
        #endregion

        public override int GetHashCode()
        {
            return imagePtr.ToInt32();
        }
    }
}
